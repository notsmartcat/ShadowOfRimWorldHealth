using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class ILHooks
{
    public static void Apply()
    {
        IL.ArenaGameSession.SpawnPlayers += ILArenaGameSessionSpawnPlayers;
    }

    static void ILArenaGameSessionSpawnPlayers(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdfld<ArenaGameSession>("chMeta"),
                x => x.MatchBrfalse(out _)
            }))
            {
                if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
                {
                x => x.MatchBr(out target)
                })){}
                else
                {
                    RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILArenaGameSessionSpawnPlayers target!");
                }
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILArenaGameSessionSpawnPlayers chMeta!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdloc(11),
                x => x.MatchLdloc(11),
                x => x.MatchLdloc(0),
                x => x.MatchLdloc(8),
                x => x.MatchCallvirt("System.Collections.Generic.List`1<ArenaSitting/ArenaPlayer>", "get_Item"),
                x => x.MatchLdfld<ArenaSitting.ArenaPlayer>("playerNumber")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldloc, 11);
                val.Emit(OpCodes.Ldloc_0);
                val.Emit(OpCodes.Ldloc, 8);
                val.EmitDelegate(ArenaGameSessionSpawnPlayers);
                val.Emit(OpCodes.Brtrue_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILArenaGameSessionSpawnPlayers!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
            {
                x => x.MatchBr(out _)
            })) { }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILArenaGameSessionSpawnPlayers skip!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdloc(11),
                x => x.MatchLdloc(11),
                x => x.MatchLdloc(0),
                x => x.MatchLdloc(8),
                x => x.MatchCallvirt("System.Collections.Generic.List`1<ArenaSitting/ArenaPlayer>", "get_Item"),
                x => x.MatchLdfld<ArenaSitting.ArenaPlayer>("playerNumber")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldloc, 11);
                val.Emit(OpCodes.Ldloc_0);
                val.Emit(OpCodes.Ldloc, 8);
                val.EmitDelegate(ArenaGameSessionSpawnPlayers2);
                val.Emit(OpCodes.Brtrue_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILArenaGameSessionSpawnPlayers!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static bool ArenaGameSessionSpawnPlayers(ArenaGameSession self, AbstractCreature abstractCreature, List<ArenaSitting.ArenaPlayer> list, int l)
    {
        abstractCreature.state = new RWPlayerHealthState(abstractCreature, list[l].playerNumber, self.characterStats_Mplayer[0].name, false);

        return true;
    }

    public static bool ArenaGameSessionSpawnPlayers2(ArenaGameSession self, AbstractCreature abstractCreature, List<ArenaSitting.ArenaPlayer> list, int l)
    {
        abstractCreature.state = new RWPlayerHealthState(abstractCreature, list[l].playerNumber, new SlugcatStats.Name(ExtEnum<SlugcatStats.Name>.values.GetEntry(list[l].playerNumber), false), false);

        return true;
    }
}
/*
class ILHooks
{
	public static bool ShadowOfLizardUpdate(Lizard self)
    {
        return true;
    }
}
*/