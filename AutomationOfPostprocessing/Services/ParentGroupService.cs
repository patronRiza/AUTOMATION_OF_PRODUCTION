using AutomationOfPostprocessing;
using NXOpen;
using NXOpen.CAE;
using NXOpen.CAM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationOfPostprocessing
{
    public class ParentGroupService
    {
        private readonly Session _session;
        private readonly NXOpen.UI _ui;
        private readonly NXLogger _logger;

        public ParentGroupService(Session session, NXLogger logger, NXOpen.UI ui)
        {
            _session = session;
            _ui = ui;
            _logger = logger;
        }

        public NCGroup GetSelectGroup()
        {
            try
            {
                int selectedCount = _ui.SelectionManager.GetNumSelectedObjects();
                if (selectedCount == 0)
                {
                    _logger.LogWarning("Нет выбранных объектов");
                    return null;
                }

                NCGroup foundGroup = null;
                int groupCount = 0;

                for (int i = 0; i < selectedCount; i++)
                {
                    try
                    {
                        TaggedObject selectedObject = _ui.SelectionManager.GetSelectedTaggedObject(i);

                        if (selectedObject is NCGroup ncGroup)
                        {
                            foundGroup = ncGroup;
                            groupCount++;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e);
                    }
                }

                if (groupCount == 0)
                {
                    _logger.LogWarning("Среди выбранных объектов нет CAM групп");
                    return null;
                }
                else if (groupCount > 1)
                {
                    _logger.LogWarning("Выбрано более одной CAM группы. Пожалуйста, выберите только одну группу.");
                    return null;
                }

                return foundGroup;
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return null;
            }
        }
    }
}

