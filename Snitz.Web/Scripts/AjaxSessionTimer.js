

      function AjaxTimeoutTimer(strLength){

        //Set all of the defaults      
        this.loaded = false;
        this.intHeight = 600;
        this.intDivHeight = 150;
        this.objDiv = null;
        this.objIFr = null;
        this.top = 0;
        this.sessionLength = 1000*60*20;
        if(typeof(strLength)!="undefined")this.sessionLength = strLength;
        var timerWarn = window.setTimeout(loadMessage,this.sessionLength);
        this.maxWait = 1000*60*5;
        this.title = "Timeout Notification";
        this.message = "Your current session is about to expire. " + 
                       "In order to remain logged in and avoid loosing " + 
                       "any data you may have entered, please press " + 
                       "the <span style='color:green'>Confirm</span> button " +
                       "to extend your session.";
        this.confirm = "Confirm";
        this.ignore = "Ignore";
        this.extendedMessage = "Your session has most likely been expired. " +
                               "Press the <span style='color:green'>Confirm</span> " +
                               "button to verify this."
        this.serverURL = null;
        var ref = this;
        
        //determine browser height and height of div
        function calcBrowserSize(){
          if(self.innerHeight){
	        ref.intHeight = self.innerHeight;
            ref.intDivHeight = ref.objDiv.scrollHeight;
          }
          else if(document.documentElement && document.documentElement.clientHeight){
	        ref.intHeight = document.documentElement.clientHeight;
            ref.intDivHeight = ref.objDiv.clientHeight;
          }
          else if(document.body){
	        ref.intHeight = document.body.clientHeight;
            ref.intDivHeight = ref.objDiv.clientHeight;
          }
          ref.intHeight = parseInt(ref.intHeight);
          ref.intDivHeight = parseInt(ref.intDivHeight);
        }


        //Start to display message on scroll
        function loadMessage(){
        
          if(ref.loaded == false)ref.loaded = createAjaxConfirm();        
          //set reference
          ref.objDiv = document.getElementById("divAjaxTimer");
          ref.objIFr = document.getElementById("ifAjaxTimer");
          
          //position block off screen and show it
          ref.objDiv.style.top = "-1000px";
          ref.objIFr.style.top = "-1000px";
          ref.objDiv.style.display = "block";
          ref.objIFr.style.display = "block";
          if(navigator.userAgent.toLowerCase().indexOf("opera") != -1)ref.objIFr.style.visibility = "hidden";
          
          //Set all of the elements text
		  document.getElementById("divAjaxBut").style.display = "block";
		  document.getElementById("divAjaxContent").innerHTML = ref.message;
		  document.getElementById("divAjaxTitle").innerHTML = ref.title;
		  document.getElementById("ajaxB1").value = ref.confirm;
		  document.getElementById("ajaxB2").value = ref.ignore;
		  
		  //position block to top edge of screen
          calcBrowserSize();
          ref.objDiv.style.top = -ref.intDivHeight + "px";
          ref.objIFr.style.top = -ref.intDivHeight + "px";
          
          //prepare block to be scrolled in
          ref.top = -ref.intDivHeight;
          timerScroll = window.setInterval(scrollIn,1);
            timerWait = window.setTimeout(extendedWait,ref.maxWait);
        }

        //Determine how much user scrolled page down
        function GetScroll(){    
          var scrollY = document.documentElement.scrollTop;
          if(!scrollY)scrollY = document.body.scrollTop;            
          if(!scrollY)scrollY = window.pageYOffset;               
          if(!scrollY)scrollY = 0;
          return scrollY;
        }
        
        //some local(private) varaibles
        var timerScroll = null;
        var timerReject = null;
        var intStep = 2;
        var intStop = Math.floor(ref.intHeight/5);
    	var timerUpdate = null;
		var intDotCount = 0;
		var timerWait = null;

        //Function moves confirmation window into view
        function scrollIn(){
          if(parseInt(intStep) + parseInt(ref.top) <= parseInt(intStop) ){
            ref.top += intStep;
            var intT = ref.top + GetScroll();;
			ref.objDiv.style.top =  intT + "px";
			ref.objIFr.style.top =  intT + "px";
          }
          else{
             var cancel = window.clearInterval(timerScroll);
             ref.objDiv.style.top = GetScroll() + intStop + "px";
             ref.objIFr.style.top = ref.objDiv.style.top;
          }
        }
        
        //Confirm button pressed, make request, hide buttons, start annimation
		this.confirmTimer = function(){
          timerUpdate = window.setInterval(updateMessage,250);
          document.getElementById("divAjaxBut").style.display = "none";
          makeHttpRequest();
		}

        //Hide information if the ignore button was clicked
		this.ignoreTimer = function(){
          ref.objDiv.style.display = "none";
          ref.objIFr.style.display = "none";
		}

        //Annimate the request waiting time so it looks fancy
		function updateMessage(){
		  var strDotsMessage = "Updating Session.";
		  for(x=0;x<intDotCount;x++)strDotsMessage+=".";
		  document.getElementById("divAjaxContent").innerHTML = strDotsMessage;
		  intDotCount++;
		  if(intDotCount>4)intDotCount = 0;
		}
		
		//change message from normal to extended info
		function extendedWait(){
		  document.getElementById("divAjaxContent").innerHTML = ref.extendedMessage;
		}
	
        //Function Prepares calls xmlHttpRequest
        function makeHttpRequest(){
        
          //Make sure we set the server side page          
          if(ref.serverURL==null){
            alert('Errror: Server Side Code was not specified by web master!');
            ref.objDiv.style.display = "none";
            ref.objIfr.style.display = "none";
            return false;
          }
          
          //create reg exp so we do not grabbed cached material
          var regEx = /(\s|:)/gi;
          var strDT = "ts=" + new Date().toString().replace(regEx,"");                              
          
          //Make the request to the server
          var loader1 = new net.ContentLoader(ref.serverURL,finishRequest,null,"POST",strDT); //Make request
        
        }
        
        //Function takes XML document 
        function finishRequest(){
          timerWarn = window.setTimeout(loadMessage,ref.sessionLength);
          var bx = window.clearTimeout(timerUpdate);
          var strDoc = this.req.responseText;  //Grab HTML
          if(strDoc.indexOf("Session Updated - Server Time:") == 0){
            document.getElementById("divAjaxContent").innerHTML = "Your session was updated sucessfully!";
            var timerHide = window.setTimeout(hideDiv,2000);
          }
          else{
            document.getElementById("divAjaxContent").innerHTML = "Warning your session has already timed out!";
            var timerHide = window.setTimeout(hideDiv,3000);
          }
        }
        
        function createAjaxConfirm(){
          var ifr = document.createElement("iframe");
      
          var div1 = document.createElement("div");
          var div2 = document.createElement("div");
          var div3 = document.createElement("div");
          var div4 = document.createElement("div");
      
          var btn1 = document.createElement("input");
          var btn2 = document.createElement("input");
      
          ifr.id = "ifAjaxTimer";
          div1.id = "divAjaxTimer";
          div2.id = "divAjaxTitle";
          div3.id = "divAjaxContent";
          div4.id = "divAjaxBut"; 
      
          btn1.type="button";
          btn2.type="button";
          btn1.id = "ajaxB1";
          btn2.id = "ajaxB2";
          btn1.className = "confirm";
          btn2.className = "ignore";
            btn1.onclick = function() {
                ajaxTimer.confirmTimer();
                return false;
            };
            btn2.onclick = function() {
                ajaxTimer.ignoreTimer();
                return false;
            };
          btn1.value = "text";
          btn2.value = "text";     
     
          div1.appendChild(div2);   
          div1.appendChild(div3);
          div4.appendChild(btn1);
          div4.appendChild(btn2);
          div1.appendChild(div4); 

          document.forms[0].appendChild(ifr);
          document.forms[0].appendChild(div1);
            
          return  true;
        }
        
        //Hide div and iframe
        function hideDiv(){
          ref.objDiv.style.display = "none";
          ref.objIFr.style.display = "none";
        }
     
      }
      function readCookie(name) {
          var nameEQ = name + "=";
          var ca = document.cookie.split(';');
          for (var i = 0; i < ca.length; i++) {
              var c = ca[i];
              while (c.charAt(0) == ' ') c = c.substring(1, c.length);
              if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
          }
          return null;
      }
//=============================================
//Update information for customization here!
//==============================================
      var sPath = window.location.pathname;
      var sPage = sPath.substring(sPath.lastIndexOf('/') + 1);
      var refresh = readCookie('ActiveRefresh');
      
      if (sPage != "active.aspx" || refresh == "0") {
          var ajaxTimer = new AjaxTimeoutTimer(8 * 60 * 1000); //default time 20 minutes
          //var ajaxTimer = new AjaxTimeoutTimer(5000);  //Specify a time length
          ajaxTimer.serverURL = "/KeepAlive.aspx";
          ajaxTimer.maxWait = 1000 * 60 * 2; //default time 5 minutes
          //ajaxTimer.title = "Timeout Notification";
          //ajaxTimer.message = "Your current session is about to expire.";
          //ajaxTimer.confirm = "Update";
          //ajaxTimer.ignore = "Cancel";
          //ajaxTimer..extendedMessage = "Your session has most likely been expired. "
      }
