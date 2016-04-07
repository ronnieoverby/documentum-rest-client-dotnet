<%@ Page Title="Demo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AspNetWebFormsRestConsumer._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    </asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <br />
   <h1>Welcome to Rest Using Asp.Net</h1>
    <h4>This is not meant to be a complex sample. It uses basic controls and uploads to show how you can integrate with DCTM Rest Services<br />
        at even the most basic level. There are toolkits that make working with Rest services very easy and you can build more complex UIs<br />
        with them.
    <p>
        <a runat="server" href="~">Home - Is this page</a> <br />
        <a runat="server" href="~/DQL">DQL - Is a page where you can enter a simple qualifer and get query results back.</a> <br />
        <a runat="server" href="~/Create">Create - This page will allow you to create an object and file it to a specific path or use a D2 Profile to automate its creation.</a> 
        </p>
    <p>
        &nbsp;</p>
    <p>
        <table style="width:100%;">
            <tr>
                <td class="auto-style2">
                    <p>
                        <asp:Label ID="lblLoginBanner" runat="server" CssClass="message-error" Text="You must Login First"></asp:Label>
                    </p>
    <p>
        UserName:&nbsp;
        <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>

        </p>
    <p>
        Password:
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>

        </p>
    <p>
        Docbase:<asp:TextBox ID="txtDocbase" runat="server">repo1</asp:TextBox>

        </p>
    <p>
        RestURL:
        <asp:TextBox ID="txtRestUrl" runat="server" Width="356px">http://10.0.12.32:8080/d2-rest/services</asp:TextBox>

        </p>
    <p>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Text="Login" />

        </p>
                </td>
                <td class="auto-style3">
        <asp:Label ID="lblUserInfo" runat="server" Visible="False"></asp:Label>

                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style3">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style3">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </p>
    <p>
    <br />
        </p>
    <p>
        &nbsp;</p>
</asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="HeadContent">
    <style type="text/css">
        .auto-style1 {
            font-size: x-large;
        }
        .auto-style2 {
            width: 559px;
        }
        .auto-style3 {
            width: 768px;
        }
    </style>
</asp:Content>

