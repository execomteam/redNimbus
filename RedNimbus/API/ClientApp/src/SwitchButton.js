import React from 'react';

export default class SwitchButton extends React.Component{
    constructor(props){
        super(props);
    }

    render(){
        return(
            <button onClick={this.ChangeFormAndValueOnClick}>{this.props.buttonValue}</button>
        )
    }

    ChangeFormAndValueOnClick(){
       // this.props.changeForm({this.props.formToShow});
        this.changeButtonValue();
    }
}