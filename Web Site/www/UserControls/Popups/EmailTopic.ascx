<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailTopic.ascx.cs"
    Inherits="SnitzUI.UserControls.Popups.EmailTopic" %>
<div id="divContactForm">
    <fieldset id="ContactFieldset">
        <label for="NameTextBox">
            <%= Resources.webResources.lblName %></label>
        <input type="text" id="ToName" /><br />
        <label for="EmailTextBox">
            <%= Resources.webResources.lblEmail %></label>
        <input type="text" id="ToEmail" /><br />
        <label for="MessageTextBox">
            <%= Resources.webResources.lblEmailMessage %></label>
        <textarea id="MessageTextBox" rows="10" cols="45"><%= Message %></textarea><br />
        <input type="hidden" id="SubjectTextBox" value="" />
        <button onclick="SendForm();return false;" type="button">
            <%= Resources.webResources.btnSend %></button>&nbsp;
        <button onclick="mainScreen.CancelModal();return false;" type="button">
                <%= Resources.webResources.btnCancel %></button>
    </fieldset>
</div>
<div id="divEmailSent" style="display: none;">
</div>
<br style="line-height: 0.5em" />
