<%@ Page Title="DQL" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DQL.aspx.cs" Inherits="AspNetWebFormsRestConsumer.DQL" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>Qualifier Query<br />
            <span class="auto-style1">Provide only the type and criteria, where the type should be dm_sysobject or sub types (e.g. dm_document where folder(&#39;/Temp&#39;)</span><br />
        </h1>
    </hgroup>

    <article>
        <p id="Query">        
           DQL Qualifier:<br />
        <asp:TextBox ID="txtQuery" runat="server" Width="1390px">dm_document where folder(&#39;/Temp&#39;)</asp:TextBox>

            <br />

            <asp:Button ID="btnExecuteQuery" runat="server" Text="Execute Query" Height="32px" OnClick="btnExecuteQuery_Click" Width="294px" />

        </p>
        <p>        
        <asp:GridView ID="gridView" runat="server" EmptyDataText="No Results"/>

        </p>

    </article>
    </asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="HeadContent">
    <style type="text/css">
    .auto-style1 {
        font-size: medium;
        font-weight: normal;
        color: #0066FF;
    }
</style>
</asp:Content>
