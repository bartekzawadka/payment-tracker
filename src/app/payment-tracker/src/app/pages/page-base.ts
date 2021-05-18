import {AlertController, LoadingController} from '@ionic/angular';
import {Observable} from 'rxjs';

export abstract class PageBase {
  protected constructor(
    protected loadingController: LoadingController,
    protected alertController: AlertController) {
  }

  async callWithLoader<T>(action: () => Observable<T>) {
    const loading = await this.loadingController.create();
    await loading.present();

    try {
      return await action().toPromise();
    } catch (error) {
      throw error;
    } finally {
      await loading.dismiss();
    }
  }

  async showError(error: any, title?: string){
    let message = this.parseError(error);
    if(!message){
      console.log(error);
      message = 'Więcej szczegółów w logach';
    }

    const alert = await this.alertController.create({
      header: title,
      message,
      buttons: ['OK']
    });
    await alert.present();
  }

  protected parseError(data: any): string {
    if(data && data.error && data.error.errors && Array.isArray(data.error.errors) && data.error.errors.length === 1){
      return data.error.errors[0];
    }

    return undefined;
  }
}
