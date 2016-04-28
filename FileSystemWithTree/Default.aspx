<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FileSystemWithTree._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <asp:TextBox ID="TextBox1" runat="server" Width="586px">Data Source=ivy-pc\sqlexpress;Initial Catalog=FileSystemTree;Integrated Security=True</asp:TextBox>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Upload file in current folder" Font-Size="Small" Height="30px" Width="266px" />
        <asp:FileUpload ID="FileUpload1" runat="server" Height="30px" Width="253px" />
        <asp:Button ID="Button2" runat="server" Text="Create new folder in current folder" OnClick="Button2_Click" Font-Size="Medium" Height="30px" Width="478px" />
&nbsp;<asp:Button ID="Button5" runat="server" Text="Search file" Font-Size="Medium" Height="30px" OnClick="Button5_Click" />
        <br />
        <asp:Label ID="Label1" runat="server"></asp:Label>
        <br />
        <asp:Panel ID="Panel1" runat="server" Height="154px" ScrollBars="Auto" Width="649px">
            <asp:TreeView ID="TreeView1" runat="server" OnSelectedNodeChanged="TreeView1_SelectedNodeChanged" Width="269px">
            </asp:TreeView>
        </asp:Panel>
        <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="Read connection string" Font-Size="Medium" Height="30px" />
        <br />
        <asp:Button ID="Button6" runat="server" Font-Size="Medium" Height="30px" OnClick="Button6_Click" Text="Enter path to physically store files" Width="340px" />
        &nbsp;<asp:TextBox ID="TextBox2" runat="server" Width="147px">D:\App_Data\</asp:TextBox>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    </div>

    </asp:Content>
