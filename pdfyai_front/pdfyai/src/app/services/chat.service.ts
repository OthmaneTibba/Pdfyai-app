import { HttpClient, HttpEventType } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BASE_URL } from '../env/constants';
import { Observable } from 'rxjs';
import { DocumentModel } from '../models/document';
import { ChatModel } from '../models/chat';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  http: HttpClient = inject(HttpClient);

  createChat(docId: string): Observable<DocumentModel> {
    return this.http.post<DocumentModel>(
      BASE_URL + 'create-chat/' + docId,
      null
    );
  }

  healthCheck() {
    return this.http.get(BASE_URL + 'health-check');
  }

  sendMessage(chatId: string, message: string) {
    const eventSource = new EventSource(
      BASE_URL + 'send-message/' + chatId + '?message=' + message,
      { withCredentials: true }
    );

    return new Observable((observer) => {
      eventSource.onopen = (event) => {
        console.log('opened ' + event.type);
      };

      eventSource.onmessage = (event) => {
        observer.next(event.data);
      };

      eventSource.onerror = () => {
        observer.complete();
        eventSource.close();
      };

      () => eventSource.close();
    });
  }

  getAllChats(): Observable<DocumentModel[]> {
    return this.http.get<DocumentModel[]>(BASE_URL + 'chats');
  }

  getChatById(chatId: string): Observable<ChatModel> {
    return this.http.get<ChatModel>(BASE_URL + 'chats/' + chatId);
  }
}
