﻿using RotationSolver.Actions;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Updaters;
#if DEBUG
namespace RotationSolver.Rotations.RangedMagicial.BLU;

internal class BLU_25 : BLU_Base
{
    public override string GameVersion => "6.3";

    public override string RotationName => "25";

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        if (TripleTrident.OnSlot && TripleTrident.RightType && TripleTrident.WillHaveOneChargeGCD(OnSlotCount(Whistle, Tingle), 0))
        {
            if (Whistle.CanUse(out act)) return true;

            if (!Player.HasStatus(true, StatusID.Tingling)
                && Tingle.CanUse(out act, mustUse: true)) return true;
            if (Offguard.CanUse(out act)) return true;

            if (TripleTrident.CanUse(out act, mustUse: true)) return true;
        }

        if (SonicBoom.CanUse(out act)) return true;
        if (DrillCannons.CanUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        if (WhiteWind.CanUse(out act, mustUse: true)) return true;
        return base.HealAreaGCD(out act);
    }
}
#endif