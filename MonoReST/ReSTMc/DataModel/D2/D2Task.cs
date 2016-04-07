using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel.D2
{
    [DataContract(Name = "Task", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class D2Task : Linkable, Executable
    {
        [DataMember(Name = "task_id")]
        public String TaskID { get; set; }

        [DataMember(Name = "sender")]
        public String Sender { get; set; }

        [DataMember(Name = "workflow_id")]
        public String WorkflowID { get; set; }

        [DataMember(Name = "date_sent")]
        public String DateSent { get; set; }

        [DataMember(Name = "date_format")]
        public String DateFormat { get; set; }

        [DataMember(Name = "supervisor_name")]
        public String SupervisorName { get; set; }

        [DataMember(Name = "task_subject")]
        public String TaskSubject { get; set; }

        [DataMember(Name = "task_instructions")]
        public String TaskInstructions { get; set; }

        [DataMember(Name = "manual_acquisition_task")]
        public bool ManualAcquisition { get; set; }

        [DataMember(Name = "task_requirements")]
        public TaskRequirements TaskRequirements { get; set; }


        [DataMember(Name = "signature_for_each_doc")]
        public bool SignatureForEachDoc { get; set; }

        [DataMember(Name = "check_lifecycle")]
        public bool CheckLifecycle { get; set; }

        [DataMember(Name = "task_context_menu_labels")]
        public TaskContextMenuLabels TaskContextMenuLabels { get; set; }

        [DataMember(Name = "can_accept")]
        public bool CanAccept { get; set; }

        [DataMember(Name = "can_reject")]
        public bool CanReject { get; set; }

        [DataMember(Name = "can_abort")]
        public bool CanAbort { get; set; }

        [DataMember(Name = "can_delegate")]
        public bool CanDelegate { get; set; }

        [DataMember(Name = "is_acquired")]
        public bool IsAcquired { get; set; }

        [DataMember(Name = "is_read")]
        public bool IsRead { get; set; }

        [DataMember(Name = "audit_info_addon")]
        public String AuditInfoAddon { get; set; }

        [DataMember(Name = "task_routed_objects")]
        public List<TaskRoutedObjects> TaskRoutedObjects { get; set; }

        [DataMember(Name = "can_addnote")]
        public bool CanAddNote { get; set; }

        [DataMember(Name = "task_name")]
        public String TaskName { get; set; }

        [DataMember(Name = "task_state")]
        public String TaskState { get; set; }

        [DataMember(Name = "task_status")]
        public String TaskStatus { get; set; }

        [DataMember(Name = "task_priority")]
        public int TaskPriority { get; set; }

        [DataMember(Name = "process_name")]
        public String ProcessName { get; set; }


        private RestController _client;
        public void SetClient(RestController client)
        {
            _client = client;
        }

        /// <summary>
        /// Rest controler client 
        /// </summary>
        public RestController Client
        {
            get { return _client; }
            set { this._client = value; }
        }


    }

    [DataContract(Name = "task_context_menu_labels")]
    public class TaskContextMenuLabels
    {
        [DataMember(Name = "label-accept")]
        public String LabelAccept { get; set; }

        [DataMember(Name = "label_reject")]
        public String LabelReject { get; set; }


    }
    [DataContract(Name = "task_routed_objects")]
    public class TaskRoutedObjects
    {
        [DataMember(Name = "id")]
        public String Id { get; set; }

        [DataMember(Name = "r_object_type")]
        public String ObjectType { get; set; }

        [DataMember(Name = "a_content_type")]
        public String ContentType { get; set; }
    }

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
