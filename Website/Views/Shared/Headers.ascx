<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Spark3.Models" %>

    <link rel="stylesheet" media="screen,print" type="text/css" href="<%=GlobalModel.PublicPathVirtualFilename("/style/layout.css")%>" />
    <!--[if gte IE 7]><link rel="stylesheet" media="screen" type="text/css" href="<%=GlobalModel.PublicPathVirtualFilename("/style/ie7styles.css")%>" /><![endif]-->
    <!--[if IE 6]><link rel="stylesheet" media="screen" type="text/css" href="<%=GlobalModel.PublicPathVirtualFilename("/style/ie6styles.css")%>" /><![endif]-->
    <link rel="stylesheet" media="screen,print" type="text/css" href="<%=GlobalModel.PublicPathVirtualFilename("/style/spark.css")%>" />
    <link rel="stylesheet" media="print" type="text/css" href="<%=GlobalModel.PublicPathVirtualFilename("/style/print.css")%>" />
    
    <link rel="icon" href="<%=GlobalModel.PublicPathVirtualFilename("/style/favicon.ico")%>" type="image/x-icon" />
    <link rel="shortcut icon" href="<%=GlobalModel.PublicPathVirtualFilename("/style/favicon.ico")%>" type="image/x-icon" />
    <script type="text/javascript" src="<%=GlobalModel.PublicPathVirtualFilename("/js/jquery-1.4.2.min.js")%>"></script>
    <script type="text/javascript" src="<%=GlobalModel.PublicPathVirtualFilename("/js/arrange.min.js")%>"></script>

    <!-- this if for the RSS feed support -->
    <script src="http://www.google.com/jsapi/"  type="text/javascript"></script>


    <!-- Analytics embeds and functions  -->
    <!-- End Analytics external embeds -->
    
   <style type="text/css">
       
    #feed-control 
    {
        padding-left: 40px;
    }
  
    span.title 
    {
    	font-weight: bold;
    	font-size: 1.1em;
    }
    
    li.no-bullet 
    {
    	list-style-type: none;
    }
    
    table.people-list 
    {
    	border: 1px solid grey;
    	display:none;
    	/*width: 500px; */
    }
    
    table.people-list thead 
    {
    	font-weight: bold;
    }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {

            $("a.show-hide").click(function () {
                $(this).siblings("table.people-list").slideToggle('slow');
            });

        });
    </script>