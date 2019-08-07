import React from 'react';

function LoginForm() {
    return (
        <div class="loginForm">
            <form action="#" method="post">
                <table>
                    <tr><td>E-mail:</td>            <td><input type="text" name="email" placeholder="e-mail adress" required /><br></br></td></tr>
                    <tr><td>Password:</td>          <td><input type="password" name="password" placeholder="Password" required /><br></br></td></tr>
                    <tr><td><input type="submit" value="Login" /></td></tr>
                </table>
            </form>
        </div>
    );
}

export default LoginForm;