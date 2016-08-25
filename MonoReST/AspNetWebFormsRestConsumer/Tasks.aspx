<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Tasks.aspx.cs" Inherits="AspNetWebFormsRestConsumer.Tasks" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>D2 Tasks<br />
            <span class="auto-style1">This demo works on D2 repository only</span><br />
            <p class="auto-style2">
                Note:
                <asp:Label ID="NoteLabel" runat="server"></asp:Label>
            </p>
        </h1>
    </hgroup>

    <article>      
        <asp:GridView ID="taskGrid" runat="server" Height="318px" Width="755px" EmptyDataText="No Tasks" BackColor="White" BorderColor="White" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="1" GridLines="None">
            <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
            <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#E7E7FF" />
            <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
            <RowStyle BackColor="#DEDFDE" Font-Size="Smaller" ForeColor="Black" />
            <SelectedRowStyle BackColor="#9471DE" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#594B9C" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#33276A" />
        </asp:GridView>
    </article>
    </asp:Content>
  <asp:Content ID="Content1" runat="server" contentplaceholderid="HeadContent">
      <style type="text/css">
    .auto-style1 {
        font-size: medium;
        font-weight: normal;
        color: #0066FF;
    }
    .auto-style2 {
        font-weight: normal;
        font-size: medium;
    }
</style>
</asp:Content>

  