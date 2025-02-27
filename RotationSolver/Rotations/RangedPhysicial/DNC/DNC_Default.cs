using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Updaters;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Rotations.RangedPhysicial.DNC;

internal sealed class DNC_Default : DNC_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.DefenseArea, $"{ShieldSamba}"},
        {DescType.HealArea, $"{CuringWaltz}, {Improvisation}"},
        {DescType.MoveAction, $"{EnAvant}"},
    };

    private protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (CuringWaltz.CanUse(out act, emptyOrSkipCombo: true)) return true;
        if (Improvisation.CanUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (ShieldSamba.CanUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= 15)
        {
            if (StandardStep.CanUse(out var act, mustUse: true)) return act;
            if (ExcutionStepGCD(out act)) return act;
        }
        return base.CountDownAction(remainTime);
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;
        //����״̬��ֹʹ��
        if (IsDancing) return false;

        //����֮̽��
        if (Devilment.CanUse(out act))
        {
            if (InBurst && !TechnicalStep.EnoughLevel) return true;

            if (Player.HasStatus(true, StatusID.TechnicalFinish)) return true;
        }

        //Ӧ�������
        if (UseClosedPosition(out act)) return true;

        //�ٻ�
        if (Flourish.CanUse(out act)) return true;

        //���衤��
        if (FanDance3.CanUse(out act, mustUse: true)) return true;

        if (Player.HasStatus(true, StatusID.Devilment) || Feathers > 3 || !TechnicalStep.EnoughLevel)
        {
            //���衤��
            if (FanDance2.CanUse(out act)) return true;
            //���衤��
            if (FanDance.CanUse(out act)) return true;
        }

        //���衤��
        if (FanDance4.CanUse(out act, mustUse: true))
        {
            if (TechnicalStep.EnoughLevel && TechnicalStep.IsCoolingDown && TechnicalStep.WillHaveOneChargeGCD()) return false;
            return true;
        }

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //�����
        if (!InCombat && !Player.HasStatus(true, StatusID.ClosedPosition1) && ClosedPosition.CanUse(out act)) return true;

        //�����貽
        if (FinishStepGCD(out act)) return true;

        //ִ���貽
        if (ExcutionStepGCD(out act)) return true;

        //�����貽
        if (InBurst && InCombat && TechnicalStep.CanUse(out act, mustUse: true)) return true;

        //����GCD
        if (AttackGCD(out act, Player.HasStatus(true, StatusID.Devilment))) return true;

        return false;
    }

    /// <summary>
    /// ����GCD
    /// </summary>
    /// <param name="act"></param>
    /// <param name="breaking"></param>
    /// <returns></returns>
    private bool AttackGCD(out IAction act, bool breaking)
    {
        act = null;
        //����״̬��ֹʹ��
        if (IsDancing) return false;

        //����
        if ((breaking || Esprit >= 85) && SaberDance.CanUse(out act, mustUse: true)) return true;

        //������
        if (Tillana.CanUse(out act, mustUse: true)) return true;

        //������
        if (StarfallDance.CanUse(out act, mustUse: true)) return true;

        //ʹ�ñ�׼�貽
        if (UseStandardStep(out act)) return true;

        //����AOE
        if (Bloodshower.CanUse(out act)) return true;
        if (Fountainfall.CanUse(out act)) return true;
        //��������
        if (RisingWindmill.CanUse(out act)) return true;
        if (ReverseCascade.CanUse(out act)) return true;

        //����AOE
        if (Bladeshower.CanUse(out act)) return true;
        if (Windmill.CanUse(out act)) return true;
        //��������
        if (Fountain.CanUse(out act)) return true;
        if (Cascade.CanUse(out act)) return true;

        return false;
    }

    /// <summary>
    /// ʹ�ñ�׼�貽
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseStandardStep(out IAction act)
    {
        if (!StandardStep.CanUse(out act, mustUse: true)) return false;
        if(Player.WillStatusEndGCD(2, 0, true, StatusID.StandardFinish)) return true;

        //�ȼ��������̫�಻����,��ֱ�����˻���ɶ��
        if (Level - Target.Level > 10) return false;

        //��Χû�е��˲�����
        if (!HasHostilesInRange) return false;

        //�����貽״̬�Ϳ���ȴ��ʱ���ͷ�
        if (TechnicalStep.EnoughLevel && (Player.HasStatus(true, StatusID.TechnicalFinish) || TechnicalStep.IsCoolingDown && TechnicalStep.WillHaveOneCharge(5))) return false;

        return true;
    }

    /// <summary>
    /// Ӧ�������
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseClosedPosition(out IAction act)
    {
        if (!ClosedPosition.CanUse(out act)) return false;

        //Ӧ�������
        if (InCombat && Player.HasStatus(true, StatusID.ClosedPosition1))
        {
            foreach (var friend in TargetUpdater.PartyMembers)
            {
                if (friend.HasStatus(true, StatusID.ClosedPosition2))
                {
                    if (ClosedPosition.Target != friend) return true;
                    break;
                }
            }
        }
        //else if (ClosedPosition.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private static bool FinishStepGCD(out IAction act)
    {
        act = null;
        if (!IsDancing) return false;

        //��׼�貽����
        if (Player.HasStatus(true, StatusID.StandardStep) && (Player.WillStatusEnd(1, true, StatusID.StandardStep) || CompletedSteps == 2 && Player.WillStatusEnd(1, true, StatusID.StandardFinish))
            || StandardFinish.CanUse(out _, mustUse: true))
        {
            act = StandardStep;
            return true;
        }

        //�����貽����
        if (Player.HasStatus(true, StatusID.TechnicalStep) && Player.WillStatusEnd(1, true, StatusID.TechnicalStep) || TechnicalFinish.CanUse(out _, mustUse: true))
        {
            act = TechnicalStep;
            return true;
        }

        return false;
    }
}
