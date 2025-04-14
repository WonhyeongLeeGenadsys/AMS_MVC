using System;
using System.Linq;
using AMS_MVC.Models;

namespace AMS_MVC.Services
{
    public class VCBChkScoreCalculator
    {

        public int CalculateFoldingFunction(VCBChk chk)
        {
            int scoreContactWear = GetContactWearScore(chk.ContactWearPercent);
            int scoreVacuumLeak = GetVacuumLeakScore(chk.VacuumLeakCurrent);
            int scoreContactResistance = GetContactResistanceScore(chk.ContactResistance);
            int scoreInsulation = GetInsulationResistanceScore(chk.InsulationResistance);
            int scoreHotSpot = GetHotspotScore(chk.HotSpot);
            int scorePdPattern = GetPdPatternScore(chk.PdPatternValue);
            int scoreMotorCurrent = GetMotorCurrentScore(chk.MotorCurrent);
            int scoreAccumSC = GetAccumShortCircuitCurrentScore(chk.AccumShortCircuitCurrent);
            int scoreSCCount = GetShortCircuitCountScore(chk.ShortCircuitCount);
            int scoreOperationCount = GetOperationCountScore(chk.OperationCount);
            int scoreOpenCloseTime = GetOpenCloseTimeScore(chk.OpenCloseTime);
            int scoreVisualCheck = GetVisualCheckScore(chk.VisualCheck);

            int[] scores = new int[]
            {
                scoreContactWear,
                scoreVacuumLeak,
                scoreContactResistance,
                scoreInsulation,
                scoreHotSpot,
                scorePdPattern,
                scoreMotorCurrent,
                scoreAccumSC,
                scoreSCCount,
                scoreOperationCount,
                scoreOpenCloseTime,
                scoreVisualCheck
            };

            return scores.Max();
        }


        private int GetContactWearScore(double value)
        {
            if (value < 20.0)
                return 1;
            else if (value < 30.0)
                return 2;
            else if (value < 50.0)
                return 3;
            else if (value < 100.0)
                return 4;
            else
                return 5;
        }

        private int GetVacuumLeakScore(double value)
        {
            if (value < 0.09)
                return 1;
            else if (value < 0.2)
                return 2;
            else if (value < 0.4)
                return 3;
            else if (value < 0.6)
                return 4;
            else
                return 5;
        }

        private int GetContactResistanceScore(double value)
        {
            if (value < 1.01)
                return 1;
            else if (value < 1.02)
                return 2;
            else if (value < 1.03)
                return 3;
            else if (value < 1.04)
                return 4;
            else
                return 5;
        }

        private int GetInsulationResistanceScore(double value)
        {
            if (value < 550)
                return 1;
            else if (value < 600)
                return 2;
            else if (value < 650)
                return 3;
            else if (value < 700)
                return 4;
            else
                return 5;
        }

        private int GetHotspotScore(double value)
        {
            return 1;
        }

        private int GetPdPatternScore(double value)
        {            
            return 1;
        }

        private int GetMotorCurrentScore(double value)
        {
            if (value < 1.005)
                return 1;
            else if (value < 1.01)
                return 2;
            else if (value < 1.015)
                return 3;
            else if (value < 1.02)
                return 4;
            else
                return 5;
        }

        private int GetAccumShortCircuitCurrentScore(double value)
        {
            if (value < 0.05)
                return 1;
            else if (value < 0.1)
                return 2;
            else if (value < 0.15)
                return 3;
            else if (value < 0.2)
                return 4;
            else
                return 5;
        }

        private int GetShortCircuitCountScore(double value)
        {
            if (value < 0.05)
                return 1;
            else if (value < 0.1)
                return 2;
            else if (value < 0.15)
                return 3;
            else if (value < 0.2)
                return 4;
            else
                return 5;
        }

        private int GetOperationCountScore(double value)
        {
            if (value < 0.01)
                return 1;
            else if (value < 0.02)
                return 2;
            else if (value < 0.03)
                return 3;
            else if (value < 0.04)
                return 4;
            else
                return 5;
        }

        private int GetOpenCloseTimeScore(double value)
        {
            if (value < 0.98)
                return 1;
            else if (value < 0.99)
                return 2;
            else if (value < 1.0)
                return 3;
            else if (value < 1.01)
                return 4;
            else
                return 5;
        }

        private int GetVisualCheckScore(double value)
        {
            return 1;
        }
    }
}
