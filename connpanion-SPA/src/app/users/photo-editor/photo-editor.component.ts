import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/_models/photo';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getUserPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseURL = environment.apiURL;
  currentMainPhoto: Photo;

  constructor(private authService: AuthService, private userService: UserService) { }

  ngOnInit() {
    this.initialiseUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initialiseUploader() {
    this.uploader = new FileUploader({
      url: this.baseURL + '/users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 5 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMainPhotograph: res.isMainPhotograph
        };
        this.photos.push(photo);
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      this.currentMainPhoto = this.photos.filter(p => p.isMainPhotograph === true)[0];
      this.currentMainPhoto.isMainPhotograph = false;
      photo.isMainPhotograph = true;
      this.authService.changeUserPhoto(photo.url);
      this.authService.currentUser.photoURL = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
    }, error => {
      console.log('Error at photo-editor::setMainPhoto.');
    });
  }

  deletePhoto(id: number) {
    // TODO: Alertify: Are you sure you want to delete this photo?
    this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
      this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
      console.log('Photo has been succesfully deleted.');
    });
  }

}
