﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Engine;

namespace SuperTownDefense.Objects.Components
{
    public class Gun : Weapon
    {
        public Gun(Entity e) : base(e)
        {
            
        }

        public void Fire(float angle)
        {
            Bomb b = new Bomb(Entity.StateRef, Entity.Body.Position + Entity.Render.Origin * Entity.Render.Scale, angle);
            Entity.AddEntity(b);
        }
    }
}
