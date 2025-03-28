using NXOpen;
using NXOpen.CAM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Operation = NXOpen.CAM.Operation;

namespace AutomationOfPostprocessing
{
    public class CAMGroupHelper
    {
        public static IEnumerable<NCGroup> GetSelectedGroups(NCGroup parentGroup)
        {
            CAMSetup camSetup = Session.GetSession().Parts.Work.CAMSetup;

            return camSetup.CAMGroupCollection.Cast<NCGroup>()
                .Where(g => g.GetParent() == parentGroup);
        }

        public static bool ValidateOperations(NCGroup group)
        {
            return group.GetMembers()
                .OfType<Operation>()
                .All(op => op.GetStatus() == CAMObject.Status.Complete);
        }
    }
}
