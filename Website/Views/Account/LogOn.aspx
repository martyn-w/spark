﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Spark3.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Log On
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        Please enter your username and password. Only authorised users may log in.
    </p>

    <% using (Html.BeginForm()) { %>
        <%= Html.ValidationSummary(true, "Login was unsuccessful. Please correct the errors and try again.") %>
        <div>
            <fieldset>
                <legend>Account Information</legend>
                
                <div class="editor-label">
                    <%= Html.LabelFor(m => m.UserName) %>
                </div>
                <div class="editor-field">
                    <%= Html.TextBoxFor(m => m.UserName) %>
                    <%= Html.ValidationMessageFor(m => m.UserName) %>
                </div>
                
                <div class="editor-label">
                    <%= Html.LabelFor(m => m.Password) %>
                </div>
                <div class="editor-field">
                    <%= Html.PasswordFor(m => m.Password) %>
                    <%= Html.ValidationMessageFor(m => m.Password) %>
                </div>
                
                <div class="editor-label">
                    <%= Html.CheckBoxFor(m => m.RememberMe) %>
                    <%= Html.LabelFor(m => m.RememberMe) %>
                </div>
                
                <p>
                    <input type="submit" value="Log On" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>