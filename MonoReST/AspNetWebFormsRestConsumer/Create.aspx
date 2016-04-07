<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="AspNetWebFormsRestConsumer.Contact" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1>Create Object</h1>
        <h5>You can can use standard create with filing to a Path, or specify a D2 Profile to Use</h5>
    </hgroup>

    <article>
        <p id="Create">
            Name&nbsp;
            <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
        </p>
        <p>
            Title&nbsp;
            <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
        </p>
        <p>
            Subject&nbsp;
            <asp:TextBox ID="txtSubject" runat="server"></asp:TextBox>
        </p>
        <p>
            Path (may be overridden by autofiling rules):&nbsp;
            <asp:TextBox ID="txtPath" runat="server" OnTextChanged="txtPathOrProfile_TextChanged"></asp:TextBox>
        </p>
        <p>
            Start Version <asp:TextBox ID="txtStartVersion" runat="server" ValidateRequestMode="Enabled"></asp:TextBox>
        </p>
        <p>
            <asp:FileUpload ID="fileToUpload" runat="server" /><br />
            <br />
        </p>
        <p>
            <asp:Button ID="btnCreate" runat="server" OnClick="btnCreate_Click" Text="Create" />
        </p>
        <p>
            <asp:Label ID="lblError" runat="server" Visible="False"></asp:Label>
        </p>
    </article>
</asp:Content>