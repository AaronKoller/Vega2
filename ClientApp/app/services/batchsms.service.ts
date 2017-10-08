import { Injectable } from '@angular/core';
import { RequestOptions, Headers } from '@angular/http';
import { HttpClient } from './../extenders/HttpClient';
import 'rxjs/add/operator/map';

@Injectable()
export class BatchSmsService {

  
  constructor(private http: HttpClient) {}

  sendSmsMessages(smsList) {
    return this.http.post('api/batchSms', smsList)
    .map(res => 
      res.json())
  }
  getbatchSMSPeopleProperties(){
    return this.http.get('/api/batchSms/peopleProperties')
    .map(res => 
      res.json())
  }
  }

//Papa csv parser -- http://papaparse.com/
