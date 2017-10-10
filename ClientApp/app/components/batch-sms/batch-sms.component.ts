import { DataTableModule } from 'primeng/components/datatable/datatable';
import { Headers } from '@angular/http';
import { ToastyModule, ToastyService } from 'ng2-toasty';
import { BatchSmsService } from './../../services/batchsms.service';
//PapaParse https://www.npmjs.com/package/ngx-papaparse
//PapaParse Angular 2 wrapper: https://www.npmjs.com/package/ngx-papaparse
import { PapaParseService } from 'ngx-papaparse';

import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from "@angular/router"; 

//shows service, html, component
//https://stackoverflow.com/questions/45441962/how-to-upload-a-csv-file-and-read-them-using-angular2

interface CsvObject {
  Phone: number;
  FirstName: string;
  LastName: string;
  RawMessage: string;
  HtmlMessage: string;
  MsgLength: number;
  Status: boolean;
}

interface SMSObject {
  people: any[];
  message: string;
  password: string;
  isUseMockData: boolean;
};

@Component({
  selector: 'app-batch-sms',
  templateUrl: './batch-sms.component.html',
  styleUrls: ['./batch-sms.component.css']
})

export class BatchSmsComponent implements OnInit {
  smsList: any[];
  text: any[];
  data: any[];
  error: any[]; 
  results: any[] = new Array();
  resultsOK: CsvObject[] = new Array();
  resultsBAD: CsvObject[] = new Array();
  csvData : CsvObject[] = new Array();
  selectedRow : Number;
  setClickedRow : Function;
  isError : Boolean = false;
  message : string;
  messageLength : Number;
  messageMaxLength = 160;
  csvName : string;
  password : string;
  isSMSDisabled = true;
  resultMessage: string;
  passwordError: string;
  generalError: string;
  isUseMockData = false;
  tokens: any[];
  allowedTokensString: string;
  messagePreview: string;
  cost: number;
  costPerMessage = 8;


  //

  constructor(    
    private route: ActivatedRoute,
    private router: Router,
    private ToastyService: ToastyService,
    private BatchSmsService: BatchSmsService,
    private papa: PapaParseService
  ) { 

    //set the clicked row so that we can select it
    this.setClickedRow = function(index){
      this.selectedRow = index;
      this.textAreaChange(this.message)}
  }

  ngOnInit() {
    this.BatchSmsService.getbatchSMSPeopleProperties()
    .subscribe(data => {
      this.tokens = data;
      this.allowedTokensString ="Allowed tokens: [" + this.tokens.join("], [") + "]";
    })
  }

  public resultsMessage(){
    if(this.resultsOK == undefined || this.resultsBAD == undefined){
      this.resultMessage = "";
      return;
    }   

    var resultsOKPlural = this.resultsOK.length == 1 ? "" : "es";
    this.resultMessage = this.resultsOK.length + " message" + resultsOKPlural + " sent.";
    if(this.resultsBAD.length > 0){
      var resultsBADPlural = this.resultsBAD.length == 1 ? "" : "s";
      this.resultMessage = this.resultMessage.slice(0, -1) + " -- " + this.resultsBAD.length + " error" +resultsBADPlural+"."
    }
    return;
  }

  public textAreaChange(newValue) {

    this.message = newValue;
    this.messageLength = 0;
    //If there is no message yet then exit
    if(!this.message){

      return;
    }

    this.messageLength = this.message.length;
    this.sendSMS_DisableButton();

    //create the preview message -- when there is no CSV Data
    //convert new lines to <br>
    var htmlMessage =  this.message.replace(/(?:\r\n|\r|\n)/g, '<br />');
    this.messagePreview = htmlMessage;

    //if there is CSVData then process all messages in there too
    if(!this.csvData || this.csvData.length === 0){
      return;
    }
    this.messageLength = 0;
    for(let csvRow of this.csvData){
      var returnMsg = this.previewMessage(this.tokens, htmlMessage, this.message, csvRow)
      csvRow.HtmlMessage = returnMsg.htmlMsg;
      csvRow.RawMessage = returnMsg.rawMsg;
      csvRow.MsgLength = returnMsg.rawLength;
      if(returnMsg.rawLength > this.messageLength){
        this.messageLength = returnMsg.rawLength
      }
    }

    //get the csvData from the selected ROW
    var test = Number(this.selectedRow);
    if(this.selectedRow >= 0){
      this.messagePreview = this.csvData[test].RawMessage;
    }else{
      this.messagePreview = this.csvData[0].RawMessage;
    }
  }

  public previewMessage(tokens,  htmlMessage, rawMessage, csvDataRow){
    var returnMsg = {htmlMsg: htmlMessage, rawMsg: rawMessage, rawLength: 0} 
    for (let token of this.tokens){
      // returnMsg.htmlMsg = returnMsg.htmlMsg.replace(new RegExp("\\["+token+"\\]", 'g'), "<span class='highlight'>" + csvDataRow[token] + "</span>");
      returnMsg.htmlMsg = returnMsg.htmlMsg.replace(new RegExp("\\["+token+"\\]", 'g'), "<strong>" + csvDataRow[token] + "</strong>");
      returnMsg.rawMsg = returnMsg.rawMsg.replace(new RegExp("\\["+token+"\\]", 'g'), csvDataRow[token]);
    }
    returnMsg.rawLength = returnMsg.rawMsg.length;

    return returnMsg;
  }

  public changeListener($event: any) {

    var self = this;
    self.results = new Array();;
    var file:File = $event.target.files[0];
    self.csvName = file.name;
    var myReader:FileReader = new FileReader();
    myReader.readAsText(file);
    var resultSet = [];

    myReader.onloadend = function(e){
      var data = self.papa.parse(myReader.result,{header: true});

      //Validation
      //convert data to numbers where applicapable
      for (let entry of data.data) {
        entry.Phone = Number(entry.Phone);
      }
      self.csvData = data.data;

      //remove blank phone numbers
      self.csvData = self.csvData.filter(item => item.Phone !== 0);
      self.sendSMS_DisableButton();
      self.textAreaChange(self.message)
      self.cost = self.csvData.length * self.costPerMessage;
    };
  }

  public passwordChange(){
    this.sendSMS_DisableButton()
  }

  public sendSMS_DisableButton(){
    if(this.csvData.length == undefined || this.csvData.length === 0){
      this.isSMSDisabled = true;
      return;
    }
    if(this.message == undefined || this.message.length === 0){
      this.isSMSDisabled = true;
      return
    }
    if(this.password == undefined || this.password.length === 0){
      this.isSMSDisabled = true;
      return
    }
    this.isSMSDisabled = false;
  }

  public sendSMS($event: any){
    var self = this;
    self.generalError = "";
    self.passwordError = "";
    if(self.csvData.length == 0)
      return

    var smsObject: SMSObject = {
      people:  self.csvData,
      message: self.message,
      password: self.password,
      isUseMockData: self.isUseMockData
    };
    self.BatchSmsService.sendSmsMessages(smsObject)
      .subscribe(data => {
        self.results = data;
        self.isError = false;
        self.resultsOK = [];
        self.resultsBAD = [];
        //set all statuses to OK
        for(let data of self.csvData){
          data.Status = true;
          data.RawMessage = "";
        }

        //take result message and put it on the CSVData
        for (let result of self.results) {
          for (let data of self.csvData) {
            if(data.Phone === Number(result.phone)){
              data.RawMessage = result.message;
              data.Status = false;
              self.isError = true;
            }
          }
        }

        //split results to OK and BAD arrays
        for(let data of self.csvData){
          if(data.Status){
            self.resultsOK.push(data);
          }else{
            self.resultsBAD.push(data);
          }
        }
        self.results = new Array();
        //set the custom order to have the bad items at the top
        for(let result of self.resultsBAD){
          self.results.push(result)
        }
        for(let result of self.resultsOK){
          self.results.push(result)
        }

        self.csvData = new Array();;
        self.resultsMessage();
      },err =>{
        var errorMessage = err._body
        if(errorMessage === "Invalid Password")
        {
          self.passwordError = errorMessage;
        }else{
          self.generalError = errorMessage;
        }
      })
  }
}