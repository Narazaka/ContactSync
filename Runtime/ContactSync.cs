using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace net.narazaka.vrchat.contact_sync
{
#if NET_NARAZAKA_VRCHAT_ContactSync_AvatarParametersDriver
    public class ContactSync : MonoBehaviour, IEditorOnly, net.narazaka.vrchat.avatar_parameters_driver.IParameterNameAndTypesProvider
#else
    public class ContactSync : MonoBehaviour, IEditorOnly
#endif
    {
        [SerializeField]
        public AssignGroup[] AssignGroups;

        public IEnumerable<VRCExpressionParameters.Parameter> GetParameterNameAndTypes()
        {
            var parameters = new List<VRCExpressionParameters.Parameter>();
            foreach (var assignGroup in AssignGroups)
            {
                var tagGroup = assignGroup.ContactSyncTagGroup;
                if (tagGroup == null) continue;
                var tags = tagGroup.Tags.ToDictionary(t => t.Name);
                foreach (var assign in assignGroup.CommanderAssigns.Concat(assignGroup.FollowerAssigns))
                {
                    if (!tags.TryGetValue(assign.Name, out var tag)) continue;
                    parameters.Add(new VRCExpressionParameters.Parameter
                    {
                        name = assign.ParameterName,
                        valueType = tag.ReceiverType == ContactSyncReceiverType.Proximity || tag.ReceiverType == ContactSyncReceiverType.Radial ? VRCExpressionParameters.ValueType.Float : VRCExpressionParameters.ValueType.Bool,
                        defaultValue = 0,
                        saved = false,
                        networkSynced = !assign.LocalOnly,
                    });
                }
            }
            return parameters;
        }
    }
}
