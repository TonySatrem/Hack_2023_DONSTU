const regForm = document.getElementById('loginForm')
const inpPhoneNumber = document.getElementById('phoneNumber')
const inpPassword = document.getElementById('password')

regForm.addEventListener("submit", (e) => {
  e.preventDefault()

  const phoneNumber = inpPhoneNumber.value 
  const password = inpPassword.value 

  if (!checkPhoneNumber(phoneNumber)) {
    inpPhoneNumber.classList.add('invalid')
    return
  } 
  else {
    inpPhoneNumber.classList.remove('invalid')
  }

  if (!checkPassword(password)) {
    inpPassword.classList.add('invalid')
    return
  } 
  else {
    inpPassword.classList.remove('invalid')
  }

  const data = {
    phoneNumber: phoneNumber,
    password: password
  }

  fetch("api/login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(data)
  })
})

function checkPhoneNumber (phoneNumber) {
  const regExp = /^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$/
  return regExp.test(phoneNumber)
}

function checkPassword (password) {
  return password.length >= 8
}