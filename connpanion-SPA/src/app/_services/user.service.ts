import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseURL = environment.apiURL;

  constructor(private http: HttpClient) { }

  getUsers(page?, itemsPerPage?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    let params = new HttpParams();
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (likesParam === 'Likers') {
      params = params.append('likers', 'true');
    }

    if (likesParam === 'Likees') {
      params = params.append('likees', 'true');
    }

    return this.http.get<User[]>(this.baseURL + '/users', {observe: 'response', params}).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseURL + '/users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseURL + '/users/' + id, user);
  }

  setMainPhoto(userID: number, photoID: number) {
    return this.http.post(this.baseURL + '/users/' + userID + '/photos/' + photoID + '/setMain', {});
  }

  deletePhoto(userID: number, photoID: number) {
    return this.http.delete(this.baseURL + '/users/' + userID + '/photos/' + photoID);
  }

  sendLike(from: number, to: number) {
    return this.http.post(this.baseURL + '/users/' + from + '/like/' + to, {});
  }

  getMessages(id: number, page?, itemsPerPage?, messageContainer?) {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams();
    params = params.append('MessageContainer', messageContainer);

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http.get<Message[]>(this.baseURL + '/users/' + id + '/messages', {observe: 'response', params})
      .pipe(map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }

        return paginatedResult;
      }));
  }

  getMessageThread(senderID: number, receiverID: number) {
    return this.http.get<Message[]>(this.baseURL + '/users/' + senderID + '/messages/thread/' + receiverID);
  }

  sendMessage(from: number, message: Message) {
    return this.http.post(this.baseURL + '/users/' + from + '/messages', message);
  }

  deleteMessage(id: number, userID: number) {
    return this.http.post(this.baseURL + '/users/' + userID + '/messages/' + id, {});
  }

  markAsRead(id: number, userID: number) {
    return this.http.post(this.baseURL + '/users/' + userID + '/messages/' + id + '/read', {})
      .subscribe();
  }
}
