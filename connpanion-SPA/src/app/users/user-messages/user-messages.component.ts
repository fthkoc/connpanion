import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-user-messages',
  templateUrl: './user-messages.component.html',
  styleUrls: ['./user-messages.component.css']
})
export class UserMessagesComponent implements OnInit {
  @Input() receiverId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService.getMessageThread(currentUserId, this.receiverId)
      .pipe(
        tap(messages => {
          for (let i = 0; i < messages.length; ++i) {
            if (messages[i].isRead === false && messages[i].receiverID === currentUserId) {
              this.userService.markAsRead(messages[i].id, currentUserId);
            }
          }
        })
      )
      .subscribe(messages => {
      this.messages = messages;
    }, error => {
      console.log('Error! UserMessagesComponent::loadMessages()' + error);
    });
  }

  sendMessage() {
    this.newMessage.receiverId = this.receiverId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe((message: Message) => {
        this.messages.unshift(message);
        this.newMessage.content = '';
      }, error => {
        console.log('Error! UserMessagesComponent::sendMessage()' + error);
      });
  }
}
