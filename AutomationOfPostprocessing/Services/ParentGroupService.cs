using NXOpen;
using NXOpen.CAM;
using System;

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
                    _logger.LogWarning("Вы не выбрали установ");
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

                if (groupCount > 1)
                {
                    _logger.LogWarning("Выбрано более одного установа. Пожалуйста, выберите только одну группу.");
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

