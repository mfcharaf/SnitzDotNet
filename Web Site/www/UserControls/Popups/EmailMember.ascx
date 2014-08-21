<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailMember.ascx.cs" Inherits="SnitzUI.UserControls.Popups.EmailMember" %>
<div style="position: relative">
    <div id="loadergif" style="position:absolute;top:0px;left:0px; width:100%;height:100%;background:#666;filter: alpha(opacity=80);-moz-opacity:.8; opacity:.8;z-index:5000;display:none;"  >
        <img src="/images/ajax-loader.gif" style="position:relative; top:45%;left:45%;" />
    </div>
    <div id="divContactForm">
        <fieldset id="ContactFieldset">
            <label><%= Resources.webResources.lblName %></label><span><%= this.MemberName %></span>
            <input type="hidden" id="ToName" value="<%= this.MemberName %>" />
            <input type="hidden" id="ToEmail" value="" />
    <br /><label for="SubjectTextBox">
                <%= Resources.webResources.lblSubject %></label>
            <input type="text" id="SubjectTextBox" /><br />
            <label for="MessageTextBox">
                <%= Resources.webResources.lblEmailMessage %></label><br />
            <textarea id="MessageTextBox" rows="10" cols="85"></textarea><br />
            
            <button onclick="SendForm();return false;" type="button">
                <%= Resources.webResources.btnSend %></button>&nbsp;
            <button onclick="mainScreen.CancelModal();return false;" type="button">
                    <%= Resources.webResources.btnCancel %></button>
        </fieldset>
    </div>
    <div id="resultText" style="color:red;display:none;"></div>
    <br style="line-height: 0.5em" />
</div>