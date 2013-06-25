<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Spark3.Models.PersonModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadTitle" runat="server">
<%= Model.HeadTitle %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server">
<%= Model.PageTitle %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<%= Model.TransformXml() %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Breadcrumbs" runat="server">
<%= Model.BreadcrumbsHtml() %>
</asp:Content>
