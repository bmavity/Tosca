<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Tosca.Core.Web.ViewModels.WelcomeEmailViewModel>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Welcome</title>
</head>
<body>
    <div>
        <p>Hello <%=Model.FirstName %></p>
    </div>
</body>
</html>
