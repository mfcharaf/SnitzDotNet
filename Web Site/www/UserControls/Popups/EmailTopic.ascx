<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailTopic.ascx.cs"
    Inherits="SnitzUI.UserControls.Popups.EmailTopic" %>
<div style="position: relative">
    <div id="loadergif" style="position:absolute;top:0px;left:0px; width:100%;height:100%;background:#666;filter: alpha(opacity=80);-moz-opacity:.8; opacity:.8;z-index:5000;display:none;"  >
        <img src="/images/ajax-loader.gif" style="position:relative; top:45%;left:45%;" />
    </div>
    <div id="divContactForm">
        <fieldset id="ContactFieldset">
            <label for="ToName">
                <%= Resources.webResources.lblName %></label>
            <input type="text" id="ToName" /><br />
            <label for="ToEmail">
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
    <div id="resultText" style="color:red;display:none;"></div>
    <br style="line-height: 0.5em" />
</div>
