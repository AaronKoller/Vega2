import {Injectable} from '@angular/core';
import {Http, Headers, RequestOptions} from '@angular/http';


//Source: http client wraper: https://stackoverflow.com/questions/34464108/angular2-set-headers-for-every-request
@Injectable()
export class HttpClient {

  constructor(private http: Http) {
  }

  get(url) {
    let headers = new Headers();
    return this.http.get(url, {
      headers: headers
    });
  }

  post(url, data) {
    let headers = new Headers();
    headers.append('Accept','application/json');
    headers.append('Content-Type','application/json');
    let options = new RequestOptions({headers: headers});
    return this.http.post(url, JSON.stringify(data), options);
  }
}