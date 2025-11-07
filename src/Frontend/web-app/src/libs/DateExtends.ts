Date.prototype.toShortTime = function (): string {
  const date = this
  const month = date.getMonth() + 1
  const monthText = month < 10 ? `0${month}` : month.toString()

  const day = date.getDate()
  const dayText = day < 10 ? `0${day}` : day.toString()

  const hours = date.getHours()
  const hoursText = hours < 10 ? `0${hours}` : hours.toString()

  const minutes = date.getMinutes()
  const minutesText = minutes < 10 ? `0${minutes}` : minutes.toString()

  return `${monthText}-${dayText} ${hoursText}:${minutesText}`
}
