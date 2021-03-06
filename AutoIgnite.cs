#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

#endregion

namespace AutoIgnite
{

    class Program
    {
        private static SpellDataInst Ignite;
        private static Obj_AI_Hero Player;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            Ignite = Player.Spellbook.GetSpell(Player.GetSpellSlot("SummonerDot"));
            if (Ignite == null || Ignite.Slot == SpellSlot.Unknown)
                return;

            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!CanIgnite())
                return;
            int dmg = 50 + 20 * Player.Level;
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero != null && hero.IsValid && hero.IsVisible && !hero.IsDead && hero.Health < dmg && Player.ServerPosition.Distance(hero.ServerPosition) <= 600))
                CastIgnite(hero);
        }

        private static bool CanIgnite()
        {
            return Ignite != null && Ignite.Slot != SpellSlot.Unknown && Ignite.State == SpellState.Ready &&
               Player.CanCast;
        }

        private static void CastIgnite(Obj_AI_Hero enemy)
        {
            if (enemy == null || !enemy.IsValid || !enemy.IsVisible || !enemy.IsTargetable || enemy.IsDead)
                return;

            if (CanIgnite())
                Player.SummonerSpellbook.CastSpell(Ignite.Slot, enemy);
        }


    }
}
