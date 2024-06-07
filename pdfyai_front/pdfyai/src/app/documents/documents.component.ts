import { Component, OnInit, inject } from '@angular/core';
import { FileService } from '../services/file.service';
import { Subscription, catchError, switchMap, throwError } from 'rxjs';
import { HeaderComponent } from '../header/header.component';
import { MatIconModule } from '@angular/material/icon';
import { DocumentModel } from '../models/document';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChatService } from '../services/chat.service';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { UserResponseDto } from '../models/user_response_dto';

@Component({
  selector: 'app-documents',
  standalone: true,
  imports: [
    HeaderComponent,
    MatIconModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatSnackBarModule,
    FormsModule,
  ],
  templateUrl: './documents.component.html',
  styleUrl: './documents.component.css',
})
export class DocumentsComponent implements OnInit {
  fileService: FileService = inject(FileService);
  chatService: ChatService = inject(ChatService);
  isUploading: boolean = false;
  isReadingFile: boolean = false;
  router: Router = inject(Router);
  fileSub$!: Subscription;
  isLoading = false;
  isDeleting: boolean = false;
  searchQuery: string = '';
  _snackBar: MatSnackBar = inject(MatSnackBar);
  documents: DocumentModel[] = [];
  filtredList: DocumentModel[] = [];
  private userService = inject(UserService);
  private authService = inject(AuthService);

  onSearch(query: string): void {
    this.filtredList = this.documents.filter((doc: DocumentModel) =>
      doc.documentName
        .toLocaleLowerCase()
        .trim()
        .includes(query.toLowerCase().trim())
    );
  }

  getAllDocs() {
    try {
      this.isLoading = true;
      this.fileSub$ = this.chatService.getAllChats().subscribe({
        next: (document: DocumentModel[]) => {
          this.isLoading = false;
          this.documents = document;
          this.filtredList = this.documents;
        },
        error: (err) => {
          this.isLoading = false;
        },
        complete: () => {
          this.isLoading = false;
        },
      });
    } catch (e) {
      this.isLoading = false;
    }
  }

  onFilePicked(event: any) {
    if (!this.isUploading && !this.isReadingFile && !this.isDeleting) {
      let files = event.target.files;
      if (!files) return; // SHOW ERROR MESSAGE LATER
      let formData = new FormData();
      let sectedFile: File = files[0];
      formData.append('file', sectedFile);
      this.isUploading = true;

      try {
        this.fileService
          .uploadFileToS3(formData)
          .pipe(
            switchMap((doc) => {
              this.isUploading = false;
              this.isReadingFile = true;
              return this.chatService.createChat(doc.id);
            }),
            catchError((error: HttpErrorResponse) => {
              if (error.status == 400) {
                this._snackBar.open(error.error, 'Close', {
                  duration: 2500,
                });
              }
              this.isReadingFile = false;
              this.isUploading = false;
              return throwError(() => error);
            })
          )
          .subscribe({
            next: (data: DocumentModel) => {
              this.documents = [data].concat(this.documents);
              this.filtredList = this.documents;
              this.isReadingFile = false;
              this.isUploading = false;
              this._snackBar.open(
                'You document uploaded successfully',
                'close',
                {
                  duration: 3000,
                }
              );
            },
            error: (err) => {
              this.isReadingFile = false;
              this.isUploading = false;
            },
          });
      } catch (e) {
        this.isUploading = false;
        this.isReadingFile = false;
      }
    }
  }

  deleteDocumnets(document: DocumentModel) {
    if (!this.isDeleting && !this.isUploading && !this.isReadingFile) {
      this.isDeleting = true;
      this.fileService.deleteFile(document.documentId).subscribe({
        next: (rseponse: any) => {
          this.isDeleting = false;
          this._snackBar.open('File deleted successfully', 'Close', {
            duration: 2000,
          });
          this.documents = this.documents.filter(
            (doc) => doc.documentId != document.documentId
          );
          this.filtredList = this.documents;
        },
        error: (error) => {
          this.isDeleting = false;
        },
      });
    } else {
      this._snackBar.open('Please wait until the file is deleted', 'Error', {
        duration: 2000,
      });
    }
  }

  onOpenClicked(document: DocumentModel) {
    this.router.navigate(['/chat/' + document.chatId]);
  }

  ngOnInit(): void {
    if (this.userService.user() == null) {
      this.authService.getUserInfo().subscribe({
        next: (user: UserResponseDto) => {
          this.userService.user.set(user);
          this.router.navigate(['/documents']);
        },
      });
    }
    this.getAllDocs();
  }
}
