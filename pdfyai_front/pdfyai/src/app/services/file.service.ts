import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BASE_URL } from '../env/constants';
import { Observable } from 'rxjs';
import { DocumentModel } from '../models/document';
import { GenerateFileDto } from '../models/generate_file_dto';

@Injectable({
  providedIn: 'root',
})
export class FileService {
  http: HttpClient = inject(HttpClient);

  uploadFileToS3(file: FormData): Observable<DocumentModel> {
    return this.http.post<DocumentModel>(BASE_URL + 'upload', file);
  }

  deleteFile(docId: string) {
    return this.http.delete(BASE_URL + 'delete/' + docId);
  }

  downloadPdf(docId: string) {
    return this.http.get(BASE_URL + 'download/' + docId, {
      responseType: 'blob',
      observe: 'body',
    });
  }

  generateFile(model: GenerateFileDto): Observable<Blob> {
    return this.http.post(BASE_URL + 'generate', model, {
      responseType: 'blob',
      observe: 'body',
    });
  }
}
