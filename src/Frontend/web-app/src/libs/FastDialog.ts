import { app } from '@/main'

export default class FastDialog {
  public static snackbar(context: string, color: string = 'info') {
    const fun = app.config.globalProperties.$toast
    fun({ text: context, theme:'dark', snackbarProps: { location: 'top center', color: color} })
  }

  public static async infoSnackbar(context: string) {
    return FastDialog.snackbar(context, 'info')
  }

  public static async successSnackbar(context: string) {
    return FastDialog.snackbar(context, 'success')
  }

  public static async warningSnackbar(context: string) {
    return FastDialog.snackbar(context, 'warning')
  }

  public static async errorSnackbar(context: string) {
    return FastDialog.snackbar(context, 'error')
  }
}
