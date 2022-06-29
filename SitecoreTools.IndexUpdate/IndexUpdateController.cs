using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace SitecoreTools.IndexUpdate
{
    public class IndexUpdateController : Controller
    {
        StringBuilder _sb = null;
        List<ID> _forbiddenIds = new List<ID>() { Sitecore.ItemIDs.RootID, Sitecore.ItemIDs.ContentRoot };
        Database SourceDatabase { get; set; }
        Item RootItem { get; set; }
        ISearchIndex TargetIndex { get; set; }

        public string Update(string id, string database, string index, bool recursive)
        {
            if (!IsAuthenticated())
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return "Access denied.";
            }

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(database) || string.IsNullOrEmpty(index))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return "Parameters missing.";
            }

            string message = string.Empty;
            if (!SetParameters(id, database, index, out message))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return message;
            }

            _sb = new StringBuilder();
            try
            {
                if (recursive && _forbiddenIds.Contains(RootItem.ID))
                    return $"Cannot recursivley update on item {id}.";


                Log.Info($"IndexUpdater: Updating {id} in {index} from database {database}, recursive:{recursive}", this);
                _sb.AppendLine($"Database: {database}.");
                _sb.AppendLine($"Index: {index}.");
                Update(RootItem, recursive, SourceDatabase, TargetIndex);

                _sb.AppendLine("Done");
            }
            catch (Exception ex)
            {
                Log.Error("Index Update Tool: Update item failed.", ex, this);
                _sb.AppendLine(ex.ToString());
            }

            return FormatResult();
        }

        public string Delete(string id, string database, string index, bool recursive)
        {
            if (!IsAuthenticated())
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return "Access denied.";
            }

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(database) || string.IsNullOrEmpty(index))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return "Parameters missing.";
            }

            string message = string.Empty;
            if (!SetParameters(id, database, index, out message))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return message;
            }

            _sb = new StringBuilder();
            try
            {
                if (recursive)
                    _sb.AppendLine("No recursive delete available.");

                Log.Info($"Index Update Tool: Delete item {id} in {index} from database {database}.", this);
                _sb.AppendLine($"Database: {database}.");
                _sb.AppendLine($"Index: {index}.");
                Delete(RootItem, SourceDatabase, TargetIndex);

                _sb.AppendLine("Done");
            }
            catch (Exception ex)
            {
                Log.Error("Index Update Tool: Delete item failed.", ex, this);
                _sb.AppendLine(ex.ToString());
            }

            return FormatResult();
        }

        private bool IsAuthenticated()
        {
            return Sitecore.Context.User.IsAuthenticated && Sitecore.Context.User.Domain.Name == "sitecore";
        }

        private void Update(Item item, bool recursive, Database database, ISearchIndex index)
        {
            var uniqueId = new SitecoreItemUniqueId(new ItemUri(item.ID, database));
            var job = Sitecore.ContentSearch.Maintenance.IndexCustodian.UpdateItem(index, uniqueId);

            job.Start();
            _sb.AppendLine("Update job started for item " + item.Name + " " + item.ID);
            Log.Info("Index Update Tool: Update item " + item.Name + " " + item.ID, this);

            // Recurse
            if (recursive && item.Children.Count > 0)
            {
                foreach (Item child in item.Children)
                {
                    Update(child, true, database, index);
                }
            }
        }

        private void Delete(Item item, Database database, ISearchIndex index)
        {
            var itemId = new SitecoreItemId(item.ID);
            var job = Sitecore.ContentSearch.Maintenance.IndexCustodian.DeleteItem(index, itemId);
            job.Start();
            _sb.AppendLine($"Deleting item {item.ID}.");
        }

        private bool SetParameters(string id, string database, string index, out string message)
        {
            message = string.Empty;
            SourceDatabase = Sitecore.Configuration.Factory.GetDatabase(database);
            if (SourceDatabase == null)
            {
                Log.Error($"IndexUpdater: Cannot find database {database}.", this);
                message = $"Cannot find database {database}.";
                return false;
            }

            ID itemId = ID.Null;
            if (!ID.TryParse(id, out itemId))
            {
                Log.Error($"IndexUpdater: Value {id} is not a valid GUID.", this);
                message = $"Value {id} is not a valid GUID.";
                return false;
            }

            RootItem = SourceDatabase.GetItem(itemId);
            if (RootItem == null)
            {
                Log.Error($"IndexUpdater: Item {id} not found in {database}.", this);
                message = $"Item {id} not found in {database}.";
                return false;
            }

            TargetIndex = ContentSearchManager.GetIndex(index);
            if (TargetIndex == null)
            {
                Log.Error($"IndexUpdater: Index {index} not found.", this);
                message = $"Index {index} not found.";
                return false;
            }

            return true;
        }

        private string FormatResult()
        {
            return _sb.ToString();//.Replace("\r\n", "<br/>");
        }
    }
}
