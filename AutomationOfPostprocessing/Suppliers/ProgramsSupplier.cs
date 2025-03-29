using NXOpen;
using NXOpen.CAM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Operation = NXOpen.CAM.Operation;

namespace AutomationOfPostprocessing.Suppliers
{
    public class ProgramsSupplier
    {
        private readonly Session _session;

        public ProgramsSupplier(Session session)
        {
            _session = session;
        }

        public Dictionary<string, List<CAMObject>> CollectProgramData(NCGroup parent)
        {
            var programData = new Dictionary<string, List<CAMObject>>();

            foreach (NCGroup group in _session.Parts.Work.CAMSetup.CAMGroupCollection)
            {
                if (group.GetParent() == parent)
                {
                    var operations = new List<CAMObject>();

                    foreach (CAMObject member in group.GetMembers())
                    {
                        if (member is Operation)
                        {
                            //Operation operation = (Operation)member;
                            operations.Add(member);
                            _session.CAMSession.PathDisplay.ShowToolPath(member);
                        }
                    }

                    if (operations.Count > 0)
                    {
                        programData.Add(group.Name, operations);
                        _session.ListingWindow.WriteLine("Найдена программа: " + group.Name + " (" + operations.Count + " операций)");
                    }
                }
            }
            return programData;
        }
    }
}
