using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Enums
{
   public enum EmailEventTypeEnum
    {
        [Description("Submission Open For Regular Players")]
        SubmissionOpenForRegulars = 1,

        [Description("Submission Open For Spare Players")]
        SubmissionOpenForSpares,

        [Description("Submission Closed For Regular Players")]
        SubmissionClosedForRegulars,

        [Description("Submission Closed For Spare Players")]
        SubmissionClosedForSpares,

        [Description("Submission Closed")]
        SubmissionClosed,

        [Description("Roster Update")]
        RosterUpdate,

        [Description("Player Confirmed")]
        PlayerConfirmed,

        [Description("Player Unconfirmed")]
        PlayerUnConfirmed,
    }
}
