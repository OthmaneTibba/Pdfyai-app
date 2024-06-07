import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { HeaderComponent } from '../header/header.component';
import { MatButtonModule } from '@angular/material/button';
import { FileService } from '../services/file.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
@Component({
  selector: 'app-mypdfeditor',
  standalone: true,
  imports: [
    FormsModule,
    AngularEditorModule,
    HeaderComponent,
    MatButtonModule,
    MatSnackBarModule,
  ],
  templateUrl: './mypdfeditor.component.html',
  styleUrl: './mypdfeditor.component.css',
})
export class MypdfeditorComponent {
  htmlContent: string = ' ';
  fileService: FileService = inject(FileService);
  private _snackBar: MatSnackBar = inject(MatSnackBar);

  config: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: 'auto',
    placeholder: 'Enter text here...',
    translate: 'no',
    defaultParagraphSeparator: 'p',
    defaultFontName: 'Arial',
  };

  generateFile() {
    if (this.htmlContent !== '') {
      this.fileService
        .generateFile({ htmlContent: this.htmlContent })
        .subscribe({
          next: (data: Blob) => {
            let blob = data;
            let url = URL.createObjectURL(blob);
            let a = document.createElement('a');
            a.href = url;
            a.download = 'pdfyai.pdf';
            a.click();
          },
          error: (error: HttpErrorResponse) => {
            this._snackBar.open(error.error, 'Close', {
              duration: 2000,
            });
            return;
          },
        });
    } else {
      this._snackBar.open('Content cannot be null to generate a pdf', 'Close', {
        duration: 2000,
      });
      return;
    }
  }
}
