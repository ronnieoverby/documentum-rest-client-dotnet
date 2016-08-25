<%@ Page Title="Demo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AspNetWebFormsRestConsumer._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    </asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <br />
   <h1>Welcome to REST .NET Sample Using Asp.Net</h1>
    <h4 class="auto-style3">This is not meant to be a complex sample. It uses basic controls and uploads to show how you can integrate with Documentum REST Services
        at even the most basic level. There are toolkits that make working with Rest services very easy and you can build more complex UIs
        with them.</h4>
    <p>
        &nbsp;</p>

        <table style="width:100%;">
            <tr>
                <td class="auto-style2">
                    <p>
                        <asp:Label ID="lblLoginBanner" runat="server" CssClass="message-error" Text="You must Login First"></asp:Label>
                    </p>
    <p>
        UserName:&nbsp;
        <asp:TextBox ID="txtUserName" runat="server">dmadmin</asp:TextBox>

        </p>
    <p>
        Password:
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>

        </p>
    <p>
        Docbase:<asp:TextBox ID="txtDocbase" runat="server">REPO</asp:TextBox>

        </p>
    <p>
        RestURL:
        <asp:TextBox ID="txtRestUrl" runat="server" Width="356px">http://localhost:8080/dctm-rest/services</asp:TextBox>

        </p>
    <p>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Text="Login" />
<asp:Label ID="lblUserInfo" runat="server" Visible="False"></asp:Label>
        </p>
                </td>
                
            </tr>
        </table>
    <p>
    <br />
        </p>
    <p>
        &nbsp;</p>
</asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="HeadContent">
    <style type="text/css">
        .auto-style2 {
            width: 559px;
        }
        .auto-style3 {
        font-weight: normal;
        color: #0066FF;
    }
        </style>
</asp:Content>

