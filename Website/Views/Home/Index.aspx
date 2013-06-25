<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
   This is a temporary home page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <a href="<%=Spark3.Models.GlobalModel.ApplicationPathVirtual%>/departments">List of Departments</a>
 
</asp:Content>
