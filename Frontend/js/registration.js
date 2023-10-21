const regForm = document.getElementById('regForm')
const inpUsername = document.getElementById('username')
const inpSurname = document.getElementById('surname')
const inpBirthDate = document.getElementById('birthDate')
const inpPhoneNumber = document.getElementById('phoneNumber')
const inpPassword = document.getElementById('password')
const inpConfirmPassword = document.getElementById('confirmPassword')
// const butConfirmPassword = document.getElementById('confirmPassword')


regForm.addEventListener('submit', (e) => {
  e.preventDefault()

  const username = inpUsername.value
  const surname = inpSurname.value
  const birthDate = inpBirthDate.value
  const phoneNumber = inpPhoneNumber.value
  const password = inpPassword.value
  const confirmPassword = inpConfirmPassword.value

  if (!checkString(username)) {
    inpUsername.classList.add('invalid')
    return
  } 
  else {
    inpUsername.classList.remove('invalid')
  }
  
  if (!checkString(surname)) {
    inpSurname.classList.add('invalid')
    return
  }
  else
  {
    inpSurname.classList.remove('invalid')
  }

  if (!checkBirthDate(birthDate)) {
    inpBirthDate.classList.add('invalid')
    return 
  }
  else {
    inpBirthDate.classList.remove('invalid')
  }

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

  if (!checkConfirmPassword(password, confirmPassword)) {
    inpConfirmPassword.classList.add('invalid')
    return
  }
  else {
    inpConfirmPassword.classList.remove('invalid')
  }

  const data = {
    name: username, 
    surname: surname, 
    birthDate: birthDate,
    phoneNumber: phoneNumber, 
    password: password
  }

  fetch("api/users", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(data)
  })
})


function checkString (str) {
  return !!str.length
}

function checkPhoneNumber (phoneNumber) {
  const regExp = /^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$/
  return regExp.test(phoneNumber)
}

function checkBirthDate (birthDate) {
  const regExp = /^[0-9]{1,2}\.[0-9]{1,2}\.[0-9]{4}$/
  return regExp.test(birthDate)
}

function checkPassword (password) {
  return password.length >= 8
}

function checkConfirmPassword (password, confirmPassword) {
  return password == confirmPassword
}
