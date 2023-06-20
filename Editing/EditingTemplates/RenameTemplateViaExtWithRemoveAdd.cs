/*

   Copyright 2019 Esri

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       https://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

   See the License for the specific language governing permissions and
   limitations under the License.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Editing.Attributes;
using ArcGIS.Desktop.Editing.Templates;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace EditingTemplates
{
    internal class RenameTemplateViaExtWithRemoveAdd : Button
    {
        protected override async void OnClick()
        {
            MapView mapvView = MapView.Active;
            if (mapvView == null)
                return;

            //I guess which layer here should not matter so we will just get whatever the first layer to work on
            var layer = mapvView.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault();

            //it is unlikely the map did not have any feature layer but just in case
            if (layer == null)
                return;

            var template = await AddTemplate(layer, "my template");
            layer.RemoveTemplate(template);
            await AddTemplate(layer, "rename template");
        }

        private async Task<EditingTemplate> AddTemplate(FeatureLayer layer, string name)
        {
            return await QueuedTask.Run(() =>
            {

                // load the schema
                var insp = new Inspector();
                insp.LoadSchema(layer);

                // set up tags
                var tags = new[] { "Point", "tag2" };

                // default construction tool - use daml-id
                string defaultTool = "esri_editing_SketchPointTool";

                //// filter - use daml-id
                //List<string> filter = new List<string>();
                //filter.Add("esri_editing_ConstructPointsAlongLineCommand");

                // create a new CIM template  - new extension method
                return layer.CreateTemplate(name,
                    "sample description", insp, defaultTool,
                    tags);
            });
        }
    }
}

