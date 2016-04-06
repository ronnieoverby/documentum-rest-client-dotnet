using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel.D2
{
    [DataContract(Name = "task_requirements")]
    public class TaskRequirements
    {
        [DataMember(Name = "intention_required")]
        public bool IntentionRequired { get; set; }

        [DataMember(Name = "delegate_type")]
        public String DeletegateType { get; set; }

        [DataMember(Name = "next_tasks_forward")]
        public Dictionary<String, String> ForwardTasks { get; set; }

        [DataMember(Name = "signoff_forward")]
        public bool SignoffForward { get; set; }

        [DataMember(Name = "signoff_reject")]
        public bool SignoffReject { get; set; }

        [DataMember(Name = "comment_forward")]
        public bool CommentForward { get; set; }

        [DataMember(Name = "comment_reject")]
        public bool CommentReject { get; set; }

        [DataMember(Name = "delegate_assistance")]
        public String DelegateAssistance { get; set; }

    }

}
