﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Carronade {
	//All "interactive" objects in the game will be some form of actor.

	public abstract class PlayerActor : KinematicActor {
		private int damageLevel = 0;
		private int maxDamage = 0;
		public bool invuln { get; private set; } = false;
		public PlayerActor(float x, float y, float r) : base(x, y, r) {

		}
		public PlayerActor(Vector2 pos, float r) : base(pos, r) {

		}
		public int GetDamageLevel() {
			return damageLevel;
		}
		public int GetMaxDamage() {
			return maxDamage;
		}
		public float GetPercentMaxHealth() {
			if (damageLevel == 0)
				return 1.0f;
			return 1.0f - ((float)damageLevel) / ((float) (maxDamage));
		}
		public override void Initialize() {
			SetVelocity(0,0);
		}
		public void SetMaxDamage(int amount) {
			maxDamage = amount;
		}
		public void Damage(int amount) {
			if (!invuln) {
				damageLevel += amount;
				damageLevel = Math.Min(damageLevel, maxDamage);
				if (damageLevel == maxDamage)
					Perish();
			}
		}
		public void Heal(int amount) {
			damageLevel -= amount;
			damageLevel = Math.Max(damageLevel, 0);
		}
		public void SetInvuln(bool inv) {
			invuln = inv;
		}
		public void Perish() {
			//Notice. This is a terrible idea. We have made 0 guaranteee that the room we've retrieved will be of type GameRoom, however by structure of the code it SHOULD always be GameRoom. Note, SHOULD.
			//TODO: FIX THIS
			GameRoom room = (GameRoom) Game1.mainGame.GetActiveRoom();
			room.Reset();
			Game1.mainGame.SwitchRooms("MainRoom");
		}
		public override void Update(GameTime gameTime) {
			
		}
		public override void LateUpdate(GameTime gameTime) {
			Vector2 playPos = GetCenterPosition();
			foreach (var actor in GameRoom.gameRoom.actors) {
				Type t = actor.GetType();
				if (t.IsSubclassOf(typeof(EnemyActor))) {
					EnemyActor enem = (EnemyActor) actor;
					Vector2 enemPos = enem.GetCenterPosition();
					float xDist = playPos.X - enemPos.X;
					float yDist = playPos.Y - enemPos.Y;
					if (Math.Sqrt((xDist * xDist + yDist * yDist)) < 32) {
						Damage(enem.damage);
						if (invuln)
							enem.OnKilled();
						else
							enem.OnImpact();
					}
				} else if(t.IsSubclassOf(typeof(PowerupActor))) {
					PowerupActor pow = (PowerupActor) actor;
					if (pow.picked)
						continue;
					Vector2 powPos = pow.GetCenterPosition();
					float xDist = playPos.X - powPos.X;
					float yDist = playPos.Y - powPos.Y;
					if (Math.Sqrt((xDist * xDist + yDist * yDist)) < 32) {
						pow.OnPickup(this, gameTime);
					}
				} else
					continue;
			}
			base.LateUpdate(gameTime);
			if (playPos.X < -GetBounds().Width / 2)
				position = new Vector2(-GetBounds().Width / 2, position.Y);
			else if (playPos.X > Game1.mainGame.ViewPort.Width)
				position = new Vector2(Game1.mainGame.ViewPort.Width - GetBounds().Width / 2, position.Y);
			if (playPos.Y < -GetBounds().Height / 2)
				position = new Vector2(position.X, -GetBounds().Height / 2);
			else if (playPos.Y > Game1.mainGame.ViewPort.Height)
				position = new Vector2(position.X, Game1.mainGame.ViewPort.Height - GetBounds().Height / 2);
		}
	}
}