define(["sitecore", "jquery"], function (Sitecore) {
  var IndexUpdate = Sitecore.Definitions.App.extend({
    initialize: function () {
      var app = this;
      $('[data-sc-id="treeSitecore"]').click(function () {
        app.txtId.set("Value", app.treeSitecore.SelectedItemId);
      });
      $('[data-sc-id="ddlDatabase"]').change(function () {
        app.treeSitecore.set("Database", app.ddlDatabase.SelectedValue);
        app.treeSitecore.reload(app.treeSitecore.RootItemId, false);
      });
      $('[data-sc-id="txtResult"]').attr('readonly', true);
    },
    update: function () {
      var app = this;
      app.txtResult.set("text", "");
      $('[data-sc-id="btnUpdate"]').prop('disabled', true);
      $('[data-sc-id="btnDelete"]').prop('disabled', true);
      $.ajax({
        url: "/api/sitecore/indexupdate/update",
        type: "GET",
        data: { id: app.txtId.Value, database: app.ddlDatabase.SelectedValue, indexname: app.ddlIndex.SelectedValue, recursive: app.chkRecursive.IsChecked },
        context: this,
        success: function (data) {
          app.txtResult.set("Value", data);
        }
      }).done(function () {
        $('[data-sc-id="btnUpdate"]').prop('disabled', false);
        $('[data-sc-id="btnDelete"]').prop('disabled', false);
      });
    },
    delete: function () {
      var app = this;
      app.txtResult.set("text", "");
      $('[data-sc-id="btnUpdate"]').prop('disabled', true);
      $('[data-sc-id="btnDelete"]').prop('disabled', true);
      $.ajax({
        url: "/api/sitecore/indexupdate/delete",
        type: "GET",
        data: { id: app.txtId.Value, database: app.ddlDatabase.SelectedValue, indexname: app.ddlIndex.SelectedValue, recursive: app.chkRecursive.IsChecked },
        context: this,
        success: function (data) {
          app.txtResult.set("Value", data);
        }
      }).done(function () {
        $('[data-sc-id="btnUpdate"]').prop('disabled', false);
        $('[data-sc-id="btnDelete"]').prop('disabled', false);
      });
    }
  });
  return IndexUpdate;
});
