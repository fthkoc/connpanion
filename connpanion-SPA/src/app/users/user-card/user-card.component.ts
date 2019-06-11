import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.css']
})
export class UserCardComponent implements OnInit {
  @Input() user: User;

  constructor(private authService: AuthService, private userService: UserService) { }

  ngOnInit() {
  }

  sendLike(to: number) {
    this.userService.sendLike(this.authService.decodedToken.nameid, to).subscribe(data => {
      console.log('You have liked: ' + this.user.knownAs);
    }, error => {
      console.log('Error! user-card::sendLike().' + error);
    });
  }

}
