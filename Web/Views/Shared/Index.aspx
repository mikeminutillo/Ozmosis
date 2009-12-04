<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Collections.Generic.IEnumerable<Ozmosis.IEntity>>" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Reflection" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>List</h2>

<ul>
<% foreach (var item in Model)
   { %>
   <li><%:Html.ActionLink(item.ToString(), "details", new { id = item.Id })%></li>
<% } %>
</ul>

<p>
<%:Html.ActionLink("Create", "create") %>
</p>
</asp:Content>

