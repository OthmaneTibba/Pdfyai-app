import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject,
} from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { MatIconModule } from '@angular/material/icon';
import { FileService } from '../services/file.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChatService } from '../services/chat.service';
import { FormsModule } from '@angular/forms';
import { Subscription, catchError, retry, switchMap, throwError } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { ChatModel } from '../models/chat';
import { MessageModel } from '../models/message';
import { UserService } from '../services/user.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { EventMessage } from '../models/event_message';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { PdfViewerModule } from 'ng2-pdf-viewer';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [
    HeaderComponent,
    MatIconModule,
    MatProgressSpinnerModule,
    FormsModule,
    MatSnackBarModule,
    MatDialogModule,
    PdfViewerModule,
  ],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css',
})
export class ChatComponent implements OnDestroy, OnInit {
  fileService: FileService = inject(FileService);
  chatService: ChatService = inject(ChatService);
  _snackBar: MatSnackBar = inject(MatSnackBar);
  cd: ChangeDetectorRef = inject(ChangeDetectorRef);
  pdfDialog: MatDialog = inject(MatDialog);

  chatSub$!: Subscription;

  isStreamingData = false;

  isLoading = false;

  router: Router = inject(Router);
  userMessage!: string;
  @ViewChild('scrollMe') private myScrollContainer!: ElementRef;

  public bootMessage = '';

  chatId?: string;

  messages: MessageModel[] = [];

  newMessages: MessageModel[] = [];

  userService: UserService = inject(UserService);

  userImage = '';

  routeSub$!: Subscription;

  docId: string = '';
  testMessage: string[] = [];

  pdf!: string;

  subscriptions$: Subscription[] = [];

  isRealoded: boolean = false;

  activatedRouter: ActivatedRoute = inject(ActivatedRoute);

  ngOnInit(): void {
    this.isLoading = true;
    this.routeSub$ = this.activatedRouter.paramMap.subscribe((map) => {
      let chatId = map.get('id');
      this.chatId = chatId as string;
      if (chatId) {
        this.chatService.getChatById(chatId).subscribe({
          next: (chat: ChatModel) => {
            this.messages = chat.messages;

            this.docId = chat.documentUniqueName;

            this.isLoading = false;

            this.downloadFile();
          },
          error: () => {
            this.isLoading = false;
          },
          complete: () => {
            this.isLoading = false;
            let interval = setTimeout(() => {
              this.scrollToBottom();
              return () => clearInterval(interval);
            }, 10);
          },
        });
      } else {
        this.isLoading = false;
        this.router.navigate(['/documents']);
      }
    });

    this.subscriptions$.push(this.routeSub$);
  }

  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop =
        this.myScrollContainer.nativeElement.scrollHeight;
    } catch (err) {}
  }

  copyText(content: string) {
    navigator.clipboard.writeText(content);
    this._snackBar.open('copied to clipboard', 'close', {
      duration: 500,
    });
  }

  sendMessage() {
    if (!this.isStreamingData && this.chatId !== null) {
      var awnser = false;
      let messageToSend = this.userMessage;
      this.isStreamingData = true;
      this.bootMessage = '';

      let userInterval = setInterval(() => {
        this.scrollToBottom();
      }, 10);

      this.messages.push({
        messageId: '',
        content: messageToSend,
        role: 'USER',
      });
      this.userMessage = '';
      this.chatService
        .healthCheck()
        .pipe(
          switchMap((res: any) => {
            return this.chatService.sendMessage(this.chatId!, messageToSend);
          }),
          catchError((error) => {
            return throwError(() => error);
          })
        )
        .subscribe({
          next: (response: any) => {
            var value = JSON.parse(response) as EventMessage;

            if (this.isStreamingData) {
              this.isStreamingData = false;
            }
            if (awnser === false) {
              this.messages.push({
                messageId: '',
                content: '',
                role: 'BOOT',
              });
              awnser = true;
            }
            this.messages[this.messages.length - 1].content += value.Message;
          },
          error: (error) => {
            awnser = false;
            messageToSend = '';
            this.isStreamingData = false;
            clearInterval(userInterval);
          },
          complete: () => {
            if (
              awnser == false &&
              (this.messages.length == 0 || this.messages.length == 1)
            ) {
              this._snackBar.open('please try again', 'error', {
                duration: 1000,
              });
              awnser = false;
              messageToSend = '';
              this.isStreamingData = false;
              clearInterval(userInterval);
              return;
            }
            awnser = false;
            messageToSend = '';
            this.isStreamingData = false;
            clearInterval(userInterval);
          },
        });
    }
  }

  downloadFile() {
    this.fileService.downloadPdf(this.docId).subscribe({
      next: (data: Blob) => {
        this.pdf = URL.createObjectURL(data);
      },
    });
  }

  ngOnDestroy(): void {
    this.subscriptions$.forEach((sub: Subscription) => {
      sub.unsubscribe();
    });
  }
}
