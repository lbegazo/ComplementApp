import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
  user: User;
  constructor(
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
  }

  // loadUser() {
  //   // tslint:disable-next-line: no-string-literal
  //   const id = +this.route.snapshot.params['id'];
  //   this.userService.getUser(id).subscribe(
  //     (user: User) => {
  //       this.user = user;
  //     },
  //     (error) => {
  //       this.alertify.error(error);
  //     }
  //   );
  // }
}
