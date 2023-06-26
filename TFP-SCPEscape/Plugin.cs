using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using System.Linq;

namespace TFP_SCPEscape
{
    public class Plugin : Exiled.API.Features.Plugin<Config>
    {
        public override string Author => "Treeshold (aka Darcy Gaming) | I can make animal noises if it helps";

        public override string Name => "SCP Escape";

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += Server_RoundStarted;
        }

        private void Server_RoundStarted()
        {
            Timing.RunCoroutine(PlayerEscapeCheckerCoroutine(), "tfp_scpescape");
        }

        private void Server_WaitingForPlayers()
        {
            Timing.KillCoroutines("tfp_scpescape");
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines("tfp_scpescape");

            Exiled.Events.Handlers.Server.WaitingForPlayers -= Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= Server_RoundStarted;
        }

        public override void OnReloaded()
        {
            OnDisabled();
            OnEnabled();
        }

        private static IEnumerator<float> PlayerEscapeCheckerCoroutine()
        {
            while (true)
            {
                foreach (var pl in Player.List)
                {
                    var pos = pl.Position;

                    if (pos.x > 119f && pos.y < 993f && pos.z > 18f && pos.x < 132f && pos.y > 987f && pos.z < 29f)
                    {
                        TryEscapePlayer(pl);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        private static void TryEscapePlayer(Player pl)
        {
            if (pl.Role.Team == PlayerRoles.Team.SCPs)
            {
                int ciCount, ntfCount;
                ciCount = Player.List.Count(ply => ply.Role.Team == PlayerRoles.Team.ChaosInsurgency);
                ntfCount = Player.List.Count(ply => ply.Role.Team == PlayerRoles.Team.FoundationForces);

                if (ciCount == ntfCount)
                {
                    pl.Role.Set(PlayerRoles.RoleTypeId.NtfSergeant, Exiled.API.Enums.SpawnReason.Respawn, PlayerRoles.RoleSpawnFlags.All);
                }
                else if (ciCount > ntfCount)
                {
                    pl.Role.Set(PlayerRoles.RoleTypeId.ChaosRifleman, Exiled.API.Enums.SpawnReason.Respawn, PlayerRoles.RoleSpawnFlags.All);
                }
                else
                {
                    pl.Role.Set(PlayerRoles.RoleTypeId.NtfSergeant, Exiled.API.Enums.SpawnReason.Respawn, PlayerRoles.RoleSpawnFlags.All);
                }
            }
            else
            {
                pl.Hurt(5f, "<color=red>Кемперить в Escape плохо!</color>");
                pl.ShowHint("<color=red>Уходите отсюда!</color>", 2);
            }
        }
    }
}
