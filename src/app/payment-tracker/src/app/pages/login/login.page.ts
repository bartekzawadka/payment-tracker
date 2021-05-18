import {Component, OnInit} from '@angular/core';
import {PageBase} from '../page-base';
import {AlertController, LoadingController} from '@ionic/angular';
import {UserService} from "../../services/user.service";
import Login from "../../models/user/login";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
})
export class LoginPage extends PageBase implements OnInit {

  password: string;

  constructor(protected loadingController: LoadingController,
              protected alertController: AlertController,
              private userService: UserService,
              private router: Router) {
    super(loadingController, alertController);
  }

  ngOnInit() {
  }

  async login(){
    const login = new Login();
    login.password = this.password;

    try {
      await this.callWithLoader(() => this.userService.authenticate(login));
      await this.router.navigate(['/']);
    }
    catch (e){
      await this.showError(e, 'Nieudana próba logowania');
      this.password = '';
    }
  }
}
